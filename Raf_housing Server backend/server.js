import 'dotenv/config';
import express from 'express';
import cors from 'cors';
import fs from 'fs';
import bodyParser from 'body-parser'
import path from 'path';
import { fileURLToPath } from 'url';

import router from "./authRoutes.js";
import verifyToken from "./authMiddleware.js";
import pool from "./db.js";

const app = express();
app.use(cors());
app.use(express.json());
app.use(bodyParser.urlencoded({extended: true}));
app.use(bodyParser.json());

app.use("/api/auth", router);

const __filename = fileURLToPath(import.meta.url); // get the resolved path to the file
const __dirname = path.dirname(__filename); // get the name of the directory

const filePath = path.join(__dirname, process.env.TELEMETRY_FILE_NAME);
const saveFile = path.join(__dirname, process.env.SAVE_FILE_NAME);

function loadServerData() {
    if(!fs.existsSync(saveFile)) {
        return {
            rawjson: "",
            decryptedLastUpdated: 0
        }
    }

    try {
        const raw = fs.readFileSync(saveFile, "utf-8");
        return JSON.parse(raw);
    } catch (error) {
        console.error(`error Reading file`, error);
        return {
            rawjson: "",
            decryptedLastUpdated: 0
        }
    }
}

// dummy endpoint - for right now we are just returning 
app.get("/api/protected", verifyToken, async (req, res) => {
    try {
        const [rows] = await pool.query("SELECT id, username, email FROM users WHERE id = ?", [req.user.userID]);
        const user = rows[0];
        res.json({message: "This is hidden data", user});
    } catch (err) {
        console.error(err);
        res.status(500).json({message: "Internal server error"});
    }
});

app.post("/telemetry", (req, res) =>  {
    try {
        const eventData = req.body;
        
        let existingEvents = []
        // checks if the path exists
        if(fs.existsSync(filePath)) {
            const rawData = fs.readFileSync(filePath, "utf-8");
            //checking if there is any data in the file path
            if(rawData.length > 0) {
                // parsing the data into json from raw string and assigning 
                // existing events to the value
                existingEvents = JSON.parse(rawData);
            }
        }
        
        eventData.timestamp = new Date().toISOString();
        existingEvents.push(eventData);
        
        fs.writeFileSync(filePath, JSON.stringify(existingEvents, null, 2));
        return res.status(200).json({message: "Data Stored"});

    } catch (error) {
        console.error(`Internal server error: ${error}`)
    }
});

function saveServerData(serverData) {
    fs.writeFileSync(saveFile, JSON.stringify(serverData), "utf-8");
}

let serverData = loadServerData();

app.post("/sync", (req, res) => {
    const clientPlainJson = req.body.plainJson;
    if(!clientPlainJson) {
        console.error("No Json Provided");
        return res.status(400).send("No Json Provided");
    }

    try {
        const clientObj = JSON.parse(clientPlainJson);
        const clientLastUpdated = clientObj.lastUpdated || 0;

        if(clientLastUpdated > serverData.decryptedLastUpdated)
        {
            serverData.rawjson = clientPlainJson;
            serverData.decryptedLastUpdated = clientLastUpdated;
            saveServerData(serverData);
            console.log("Server Updated The Clients Data");
        } else {
            console.log("Server Info is Newer; Not Updating Client Data");
        }
        return res.send(serverData.rawjson);

    } catch(error) {
        console.error("Error in the sync request", error);
        return res.status(500);
    }
});

const PORT = process.env.PORT;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
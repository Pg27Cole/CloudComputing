import express from 'express';
const router = express.Router();
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import pool from "./db.js"

router.post("/signup", async (req, res) => {
    try {
        const { username, email, password } = req.body;

        // TODO: more validiation
        if(!username || !email || !password) {
            res.status(400).json({ message: "MISSING INFO: username/email/password" });
        }

        const [existing] = await pool.query(
            "SELECT id FROM users WHERE username = ? OR email = ?",
            [username, email]
        );
        if (existing.length > 0) {
            return res.status(400).json({message: "User of email already exists"});
        }

        const saltRounds = 10;
        const hashedPassword = await bcrypt.hash(password, saltRounds);

        await pool.query(
            "INSERT INTO users (username, email, password_hash) VALUES (?, ?, ?)",
            [username, email, hashedPassword]
        );

        res.status(201).json({message: "User created successfully."});

        // example of password hashing
        // $2b$10askfjfdabjasdgu938q43\aasdfjk,

    } catch {
        console.error(error);
        return res.status(500).json({message: "Internal server error"});
    }
});

router.post("/signin", async (req, res) => {
    try {
        const {username, password} = req.body;

        //TODO: more validation 
        if(!username || !password) {
            return res.status(400).json({message: "Missing username or password"});
        }

        const [existing] = await pool.query(
            "SELECT id, username, password_hash FROM users WHERE username = ?",
            [username]
        );

        if(existing.length === 0) {
            return res.status(401).json({message: "Invalid credentials"})
        }

        const user = existing[0]; // first and only row in the DB
        const match = await bcrypt.compare(password, user.password_hash);
        if(!match) {
            return res.status(401).json({message: "Invalid credentials"})
        }

        // keep tokens secure, otherwise hackers can just use the token to get into accounts
        const token = jwt.sign(
            { userID: user.id, username: user.username },
            process.env.JWT_SECRET,
            { expiresIn: "1d" }
        );

        return res.status(200).json({message: "Logged in", token});

    } catch(error) {
        console.error(error);
        return res.status(500).json({message: "Internal server error"});
    }
});

export default router;
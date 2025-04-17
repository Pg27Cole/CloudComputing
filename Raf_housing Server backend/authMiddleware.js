// intermediate step to validate the token passed before allowing the user to proceed
import jwt from 'jsonwebtoken';
import 'dotenv/config';

function verifyToken(req, res, next) {
    console.log("Verfying token....");
    const authHeader = req.headers["authorization"];
    if(!authHeader) {
        console.log("No Token Provided");
        return res.status(401).json({message: "No token provided"});
    }
    
    const token = authHeader.split(" ")[1];
    jwt.verify(token, process.env.JWT_SECRET, (err, decoded) =>{
        if(err) {
            console.log(`Invalid token: ${token}`);
            return res.status(401).json({message: "Invalid token"});
        }
        req.user = decoded;
        next();
    });
}

export default verifyToken;
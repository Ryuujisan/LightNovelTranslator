import axios from "axios";

export const http = axios.create({
    baseURL: "http://localhost:5156/api",
    headers: {
        "Content-Type": "application/json"
    }
});
import { http } from "../../shared/api/http";

export async function getStatus() {
    const response = await http.get("/ollama/status");
    return response.data;
}

export async function getModels() {
    const response = await http.get("/ollama/models");
    return response.data;
}
import {http} from "../../shared/api/http.ts";


export async function getOutputPath() {
    const response = await http.get("/file/output");
    return response.data;
}

export async function postResolvePath(fileName: string[]) {
    const response = await http.post("/file/resolve", {Files: fileName});
    return response.data;
}

export async function postUploadFile(file : File[]) {
    const formData = new FormData();
    file.forEach(f => formData.append("files", f));
    const response =await http.post("/file/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
}
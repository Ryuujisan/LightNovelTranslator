import {http} from "../../shared/api/http.ts";
import axios from "axios";


export async function getOutputPath() {
    const response = await http.get("/file/output");
    return response.data;
}

export async function postResolvePath(fileName: string[]) {
    const response = await http.post("/file/resolve", {Files: fileName});
    return response.data;
}


export async function postUploadFile(files : File[]) {
    const formData = new FormData();

    files.forEach(file => {
        formData.append("files", file, file.name);
    });

    const response = await axios.post(
        "http://localhost:5156/api/file/upload",
        formData
    );

    return response.data;

    return response.data;
}
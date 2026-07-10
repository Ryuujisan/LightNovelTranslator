
import type {JobFile, JobRequest, TranslationJobRequest} from "./type.ts";
import {http} from "../../shared/api/http.ts";


export async function postTranslate(job : TranslationJobRequest){
    console.log(job);
    const response = await http.post("/translate/job", job);
    return response.data;
}

export async function fetchJobs() {
    const response = await http.get("/jobs");
    return response.data;
}

export async function fetchJob(jobId: string) {
    const response = await http.get(`/jobs/${jobId}`);
    return response.data;
}

export async function deleteJob(jobId: string) {
    const response = await http.delete(`/jobs/${jobId}`);
    return response.data;
}

export async function deleteJobFiles(jobId: string, files: JobFile[]) {
    const response = await http.delete(`/jobs/${jobId}/files`, {
        data: files
    });

    return response.data;
}

export async function postStartJob(job : JobRequest) {
    const response = await http.post("/jobs/start", job);
    return response.data;
}

export async function stopJob() {
    const response = await http.post("/jobs/stop");
    return response.data;
}
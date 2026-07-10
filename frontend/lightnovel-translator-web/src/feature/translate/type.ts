export interface TranslationJobRequest {
    inputPath: string;
    outputPath: string;
    language: string;
    model: string;
    retryModel: string;
    extension : string;
}

export interface JobRequest {
    files: JobFile[];
    model: string | null;
    retryModel: string | null;
    outputPath: string | null;
    language: string | null;
    type: string;
}

export type Job = {
    id: string;
    name: string;
    inputPath: JobFile[];
}

export type JobDto = {
    id: string;
    name: string;
}

export type JobFile ={
    fileName: string;
    path: string;
    status: string;
}
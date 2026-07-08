export type UploadResponse = {
    jobId: string;
    inputDir: string;
    files: {
        fileName: string;
        path: string;
    }[];
};
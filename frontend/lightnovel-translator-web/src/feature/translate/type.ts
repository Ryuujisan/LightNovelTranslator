export interface TranslationJobRequest {
    inputPath: string;
    outputPath: string;
    language: string;
    model: string;
    retryModel: string;
    extension : string;
}
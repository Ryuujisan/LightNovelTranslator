export interface TranslationJobRequest {
    inputPath: string;
    outputPath: string;
    language: string;
    model: string;
    extension : string;
}
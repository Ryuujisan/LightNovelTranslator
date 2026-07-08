import { create } from "zustand";

type TranslationState = {
    fileNames: string[];
    resolvedPaths: string[];
    selectedModel: string;
    selectedRetryModel: string;
    language: string;
    outputPath: string;
    isTranslating: boolean;
    isLoading: boolean;
    currentChunk: number;
    totalChunks: number;

    setFileNames: (files: string[]) => void;
    setResolvedPaths: (paths: string[]) => void;
    setSelectedModel: (model: string) => void;
    setSelectedRetryModel: (model: string) => void;
    setLanguage: (language: string) => void;
    setOutputPath: (path: string) => void;
    setProgress: (current: number, total: number) => void;
    setIsTranslating: (value: boolean) => void;
    setIsLoading: (value: boolean) => void;
};

export const useTranslationStore = create<TranslationState>((set) => ({
    fileNames: [],
    resolvedPaths: [],
    selectedModel: "qwen3.5:9b",
    selectedRetryModel: "huihui_ai/Qwen3.6-abliterated:27b",
    language: "Polish",
    outputPath: "click to select",
    isTranslating: false,
    isLoading: false,
    currentChunk: 0,
    totalChunks: 1,

    setFileNames: (fileNames) => set({ fileNames }),
    setResolvedPaths: (resolvedPaths) => set({ resolvedPaths }),
    setSelectedModel: (selectedModel) => set({ selectedModel }),
    setSelectedRetryModel: (selectedRetryModel) => set({ selectedRetryModel }),
    setLanguage: (language) => set({ language }),
    setOutputPath: (outputPath) => set({ outputPath }),
    setProgress: (currentChunk, totalChunks) => set({ currentChunk, totalChunks }),
    setIsTranslating: (isTranslating) => set({ isTranslating }),
    setIsLoading : (isLoading) => set({isLoading})
}));
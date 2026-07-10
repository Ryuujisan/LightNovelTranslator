import { create } from "zustand";
import type {Job, JobFile} from "../feature/translate/type.ts";

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
    selectedJob : Job | null;
    selectableJobFiles : JobFile[];
    jobType: string;
    currentTranslation : string | null;

    setFileNames: (files: string[]) => void;
    setResolvedPaths: (paths: string[]) => void;
    setSelectedModel: (model: string) => void;
    setSelectedRetryModel: (model: string) => void;
    setLanguage: (language: string) => void;
    setOutputPath: (path: string) => void;
    setProgress: (current: number, total: number) => void;
    setIsTranslating: (value: boolean) => void;
    setIsLoading: (value: boolean) => void;
    setSelectedJob : (job: Job | null) => void;
    setJobType : (type: string) => void;
    setCurrentTranslation : (translation: string | null) => void;
    setSelectableJobFiles: (
        value: JobFile[] | ((prev: JobFile[]) => JobFile[])
    ) => void;
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
    selectedJob: null,
    selectableJobFiles: [],
    jobType: "Retry",
    currentTranslation: "",

    setFileNames: (fileNames) => set({ fileNames }),
    setResolvedPaths: (resolvedPaths) => set({ resolvedPaths }),
    setSelectedModel: (selectedModel) => set({ selectedModel }),
    setSelectedRetryModel: (selectedRetryModel) => set({ selectedRetryModel }),
    setLanguage: (language) => set({ language }),
    setOutputPath: (outputPath) => set({ outputPath }),
    setProgress: (currentChunk, totalChunks) => set({ currentChunk, totalChunks }),
    setIsTranslating: (isTranslating) => set({ isTranslating }),
    setIsLoading : (isLoading) => set({isLoading}),
    setSelectedJob : (job) => set({selectedJob : job}),
    setSelectableJobFiles: (value) =>
        set((state) => ({
            selectableJobFiles:
                typeof value === "function"
                    ? value(state.selectableJobFiles)
                    : value
        })),
    setJobType : (type) => set({jobType : type}),
    setCurrentTranslation : (translation) => set({currentTranslation : translation}),
}));
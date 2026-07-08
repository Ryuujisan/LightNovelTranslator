import {useTranslationStore} from "./tranlateStore.ts";
import * as signalR from '@microsoft/signalr';
import {create} from "zustand";
import {toast} from "react-toastify";

export interface TranslationProgressPacket {
    currentChunk: number;
    totalChunks: number;
    currentText?: string;
}
export interface TranslationCompletePacket {
    fileName : string;
}

export interface TranslationError {
    fileName: string;
    message: string;
}
interface SocketStore {
    isConnected: boolean;
    connection: signalR.HubConnection | null;
    initSignalR: () => void;
}

export const useSocketStore = create<SocketStore>((set, get) =>({
    isConnected: false,
    connection: null,
    initSignalR: () => {
        if (get().connection) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5156/hubs/translation")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        //Packet Hendler
        connection.on("Progress", (packet: TranslationProgressPacket) => {
            console.log(packet);
            useTranslationStore.getState().setProgress(packet.currentChunk, packet.totalChunks);
            useTranslationStore.getState().setIsTranslating(true);
        })

        connection.on("Complete", (packet: TranslationCompletePacket) => {
            toast.success("Translating complete" + packet.fileName);
            useTranslationStore.getState().setIsTranslating(false);
        })

        connection.on("Error", (packet: TranslationError) => {
            toast.error("Translating error " + packet.fileName)
            console.error("Translate error: " + packet.message);
            useTranslationStore.getState().setIsTranslating(false);
        })

        //Connection
        connection.start()
            .then(() => set({ isConnected: true }))
            .catch(err => console.error(err));

        connection.onreconnecting(() => set({ isConnected: false }));
        connection.onreconnected(() => set({ isConnected: true }));
        connection.onclose(() => set({ isConnected: false }));

        set({ connection });
    },

    sendCheckJob : () => get().connection?.send("CheckJob"),
}))
import { useEffect, useState } from "react";
import {Card, CardContent, Typography, Stack, Alert} from "@mui/material";
import CircleIcon from "@mui/icons-material/Circle";
import {getStatus} from "./api.ts";
import {getLatestApiReleaseUrl} from "../../shared/api/releaseUrl.ts";

export default function StatusCard() {
    const [backendOnline, setBackendOnline] = useState(false);
    const [ollamaOnline, setOllamaOnline] = useState(false);
    const [apiUrl, setApiUrl] = useState("");
    useEffect(() => {
        async function loadStatus() {
            try {
                setBackendOnline(true);

                const data = await getStatus();
                setOllamaOnline(data.installed === true);
            } catch {
                setBackendOnline(false);
                setOllamaOnline(false);
            }
        }
        async function getReleaseUrl() {
            try {
                const url = await getLatestApiReleaseUrl()
                setApiUrl(url);
            } catch {
                setApiUrl("https://api.github.com/repos/Ryuujisan/LightNovelTranslator/releases");
            }
        }
        getReleaseUrl();
        loadStatus();
    }, []);

    return (
        <Card variant="outlined">
            <CardContent>
                <Typography variant="h4">Status:</Typography>

                <Stack spacing={1}>
                    <Typography>
                        Local Backend{" "}
                        <CircleIcon color={backendOnline ? "success" : "error"} />
                    </Typography>

                    <Typography>
                        Ollama{" "}
                        <CircleIcon color={ollamaOnline ? "success" : "error"} />
                    </Typography>
                    {!backendOnline && <Alert severity="warning">
                        Local backend is not running.
                        <a
                            href = {apiUrl}
                            target="_blank"
                        >
                            Download backend
                        </a>
                    </Alert>}
                    {!ollamaOnline && <Alert severity="warning">
                        Ollama is not installed.
                        <a
                            href="https://ollama.com/download"
                            target="_blank"
                        >
                            Download Ollama
                        </a>
                    </Alert>}
                </Stack>
            </CardContent>
        </Card>
    );
}
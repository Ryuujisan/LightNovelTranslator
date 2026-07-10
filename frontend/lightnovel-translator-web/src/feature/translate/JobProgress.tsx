import {Box, Button, Stack, Typography} from "@mui/material";
import EngineeringIcon from '@mui/icons-material/Engineering';
import type {JobRequest} from "./type.ts";
import {useTranslationStore} from "../../store/tranlateStore.ts";
import {postStartJob} from "./api.ts";
import {toast} from "react-toastify";
import CancelIcon from '@mui/icons-material/Cancel';
import LinearChunkProgress from "./componets/LinearChunkProgress.tsx";

export default function JobProgress() {
    const setTranslate = useTranslationStore(x => x.setIsTranslating);
    const translate = useTranslationStore(x => x.isTranslating);
    const file = useTranslationStore(x => x.selectableJobFiles)
    const outputPath = useTranslationStore(x => x.outputPath);
    const model = useTranslationStore(x => x.selectedModel)
    const retryModel = useTranslationStore(x => x.selectedRetryModel)
    const language = useTranslationStore(x => x.language);
    const job = useTranslationStore(x => x.jobType);
    const currentTranslation = useTranslationStore(x => x.currentTranslation);

    const currentChunk = useTranslationStore(x => x.currentChunk);
    const maxChunk = useTranslationStore(x => x.totalChunks);
    async function doTranslate() {
        setTranslate(true);
        const data : JobRequest = {
            files: file,
            outputPath: outputPath,
            language: language,
            model: model,
            retryModel: retryModel,
            type: job === "Retry" ? "Start" : job

        }
        console.log(data);
        await sendRequest(data);
    }

    async function sendRequest(data : JobRequest) {
        try {
            await postStartJob(data);
            toast.success(`Queued: ${job}`)
        } catch (e) {
            console.log(e);
            toast.error("Error while translating \n" + e);
            setTranslate(false);
        }
    }
    return (
        <Box
            component="fieldset" // <--- To zmienia tag HTML na fieldset!
            sx={{
                border: '1px solid',
                borderColor: 'divider', // Używa koloru linii z motywu MUI
                borderRadius: 1,       // Zaokrąglenie rogów zgodne z MUI (np. 4px)
                padding: 3,
                //margin: 2,
                backgroundColor: 'background.paper', // Tło karty
                '& legend': {
                    padding: '0 8px',    // Daje lekki margines tekstowi, żeby linia nie dotykała liter
                    color: 'primary.main', // Kolor napisu (np. Twój ładny jasnoniebieski)
                    fontWeight: 'bold',
                    fontSize: '0.85rem',
                    textTransform: 'uppercase', // Opcjonalnie: napisy wielkimi literami wyglądają super
                },
            }}
        >
            <Typography component="legend">Progress</Typography>
            <Stack spacing={1} direction={"row"}>
                <Button variant="outlined" startIcon={<EngineeringIcon/>} loading={translate} onClick={doTranslate}>
                    Do it...
                </Button>
                <Button startIcon={<CancelIcon />} color="error" disabled={!translate}>
                    Cancel Job
                </Button>
            </Stack>
            <Stack sx={{paddingTop: 2}} spacing={1}>
                <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ mr: 1 }}
                >
                    {"Titile: " + (currentTranslation ?? "No translation")}
                </Typography>
                <Typography variant="body1">
                    Job Type: {job}
                </Typography>
                <LinearChunkProgress current={currentChunk} total={maxChunk} title={currentTranslation ?? "No translation"}/>
            </Stack>
        </Box>
    )
}

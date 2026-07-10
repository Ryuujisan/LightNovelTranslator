import {Box, Button, Stack, Typography} from "@mui/material";
import TranslateIcon from '@mui/icons-material/Translate';
import {useTranslationStore} from "../../store/tranlateStore.ts";
import type {TranslationJobRequest} from "./type.ts";
import {toast} from "react-toastify";
import {postTranslate} from "./api.ts";
import LinearChunkProgress from "./componets/LinearChunkProgress.tsx";

export default function TranslateProgress() {
    const translate = useTranslationStore(x => x.isTranslating);
    const setTranslete = useTranslationStore(x => x.setIsTranslating);
    const file = useTranslationStore(x => x.fileNames)
    const outputPath = useTranslationStore(x => x.outputPath);
    const currentChunk = useTranslationStore(x => x.currentChunk);
    const maxChunk = useTranslationStore(x => x.totalChunks);
    const model = useTranslationStore(x => x.selectedModel)
    const retryModel = useTranslationStore(x => x.selectedRetryModel)
    const language = useTranslationStore(x => x.language);
    //const sendCheckJob = useSocketStore(x => x.);

    async function doTranslete() {
        setTranslete(true);
        file.forEach((file) => {
            const data : TranslationJobRequest = {
                inputPath: file,
                outputPath: outputPath,
                language: language,
                model: model,
                retryModel: retryModel,
                extension: "json",
            }
            sendRequest(data);

        })
    }

    async function sendRequest(data : TranslationJobRequest) {
        try {
            await postTranslate(data);
            toast.success(`Queued: ${data.inputPath.split("/").pop()}`)
        } catch (e) {
            toast.error("Error while translating \n" + e);
            setTranslete(false);
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

            <Stack spacing={1}>
                <Button variant="outlined" startIcon={<TranslateIcon />} loading={translate} onClick={doTranslete}>
                    Translete...
                </Button>
                <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ mr: 1 }}
                >
                    Translate status
                </Typography>
                <LinearChunkProgress current={currentChunk} total = {maxChunk} title={""}/>
            </Stack>
        </Box>
    )
}

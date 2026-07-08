import {useEffect, useState} from "react";
import {getModels} from "../Ollama/api.ts";
import {
    Accordion,
    AccordionSummary,
    Autocomplete,
    Box,
    Button,
    Stack,
    TextField,
    Tooltip,
    Typography
} from "@mui/material";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import {getOutputPath} from "../file/api.ts";
import {useTranslationStore} from "../../store/tranlateStore.ts";
import InfoIcon from '@mui/icons-material/Info';



export function TranslateOutput() {
    /*
    * Myśl na przyszłosc na modeli zrobić zustana lub coś podobnego*/
    const [models, setModels] = useState(["Not Instaled"]);

    const outputPath = useTranslationStore(x => x.outputPath);
    const SetOutputPath = useTranslationStore(x => x.setOutputPath);
    const loading = useTranslationStore(x => x.isLoading);
    const setLoading = useTranslationStore(x => x.setIsLoading);// signir będzie prawdopodbne t aktulizwosć

    const setSelectedModel = useTranslationStore(x => x.setSelectedModel);
    const setSelectedRetryModel = useTranslationStore(x => x.setSelectedRetryModel);
    const languages = ["Polish", "English",];
    useEffect(() => {
        async function fetchModels() {
            try {
                const data = await getModels();
                setModels(data);
            } catch {
                setModels(["Not Instaled"]);
            }
        }

        fetchModels();
    }, [])

    async function selectPathOnClick() {
        setLoading(true);
        try {
            const data = await getOutputPath();
            SetOutputPath(data.path);
            setLoading(false);
        } catch (e) {
            console.log(e);
            setLoading(false);
        }
    }

    return (
        <Accordion>
            <AccordionSummary
                expandIcon={<ExpandMoreIcon/>}
                aria-controls={`output-panel1-content`}
                id={`output-panel1-header`}
            >
                <Typography component="span">Output</Typography>
            </AccordionSummary>
            <Stack spacing={1}>
                <Typography>Model:</Typography>
                <Autocomplete
                    onChange={(_, value ) => setSelectedModel(value ?? "")}
                    disablePortal
                    options={models}
                    sx={{width: 300}}
                    renderInput={(params) => <TextField {...params} label={"models"}
                    />}
                />
                <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                    <Typography>Retry Model:</Typography>

                    <Tooltip
                        title="Retry model is used when a chunk fails translation validation.
For best results choose a model that is equal or more capable than the primary model."
                        arrow
                    >
                        <InfoIcon fontSize="small" color="action" />
                    </Tooltip>
                </Box>

                <Autocomplete
                    onChange={(_, value) => setSelectedRetryModel(value ?? "")}
                    disablePortal
                    options={models}
                    sx={{ width: 300 }}
                    renderInput={(params) => (
                        <TextField {...params} label="Retry model" />
                    )}
                />

                <Typography>Languages:</Typography>
                <Autocomplete
                    disablePortal
                    options={languages}
                    sx={{width: 300}}
                    renderInput={(params) => <TextField {...params} label={languages[0]}/>}
                />
                <Typography>Output Path:</Typography>
                <Button variant="outlined" loading={loading} onClick={selectPathOnClick}>{outputPath}</Button>
            </Stack>
        </Accordion>

    )
}

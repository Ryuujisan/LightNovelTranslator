import {useEffect, useState} from "react";
import {getModels} from "../Ollama/api.ts";
import {
    Accordion,
    AccordionSummary,
    Autocomplete,
    Box,

    Stack,
    TextField,
    Tooltip,
    Typography
} from "@mui/material";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import {useTranslationStore} from "../../store/tranlateStore.ts";
import InfoIcon from '@mui/icons-material/Info';
import OutputButton from "./componets/OutputButton.tsx";



export function TranslateOutput() {

    const [models, setModels] = useState(["Not Instaled"]);
    const setSelectedModel = useTranslationStore(x => x.setSelectedModel);
    const setSelectedRetryModel = useTranslationStore(x => x.setSelectedRetryModel);
    const selectedLangue = useTranslationStore(x => x.language)
    const setSelectedLangue = useTranslationStore(x => x.setLanguage);
    const languages = ["Polish", "English", "Japanese"];
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
                    onChange={(_, value) => setSelectedLangue(value ?? "")}
                    options={languages}
                    sx={{width: 300}}
                    renderInput={(params) => <TextField {...params} label={selectedLangue}/>}
                />
                <OutputButton />
            </Stack>
        </Accordion>

    )
}

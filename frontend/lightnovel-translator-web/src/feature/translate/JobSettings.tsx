import {
    Accordion,
    AccordionSummary,
    Autocomplete,
    Button,
    Stack,
    TextField,
    Typography
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import {useEffect, useState} from "react";
import {getModels} from "../Ollama/api.ts";
import {useTranslationStore} from "../../store/tranlateStore.ts";
import SelectableTranslate from "./componets/SelectableTranslate.tsx";

export default function JobSettings() {

    const jobType = ["Retry", "Resume"]
    const [models, setModels] = useState(["Not Instaled"]);
    const job = useTranslationStore(x => x.jobType);
    const setJob = useTranslationStore(x => x.setJobType);
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
                aria-controls={`JobSettings-panel-content`}
                id={`JobSettings-panel-header`}
            >
                <Typography component="span">Job Settings</Typography>
            </AccordionSummary>

            <Stack spacing={1}>
                <Stack direction="row" spacing={1} sx={{alignItems: "center"}}>
                    <TextField id="job name" label="Job name"/>
                    <Button variant="outlined">Rename</Button>
                </Stack>
                <Autocomplete
                    onChange={(_, value ) => setJob(value ?? "")}
                    disablePortal
                    options={jobType}
                    sx={{width: 300, paddingTop: 2}}
                    renderInput={(params) => <TextField {...params} label={"Job type"}
                    />}
                />

                {job == jobType[0] && <SelectableTranslate models={models}
                                 language={languages}
                                 selectedLangue={selectedLangue}
                                 onModelChange={setSelectedModel}
                                 onRetryModelChange={setSelectedRetryModel}
                                 onLanguageChange={setSelectedLangue}/> }
            </Stack>


        </Accordion>
    )
}

import {Autocomplete, Box, Stack, TextField, Tooltip, Typography} from "@mui/material";
import InfoIcon from "@mui/icons-material/Info";
import OutputButton from "./OutputButton.tsx";

type SelectableTranslateProps = {
    models: string[];
    language: string[];
    selectedLangue: string;

    onModelChange(value: string): void;
    onRetryModelChange(value: string): void;
    onLanguageChange(value: string): void;
}
export default function SelectableTranslate(props: SelectableTranslateProps) {
    return (
        <Stack spacing={1}>
            <Typography>Model:</Typography>
            <Autocomplete
                onChange={(_, value ) => props.onModelChange(value ?? "")}
                disablePortal
                options={props.models}
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
                onChange={(_, value) => props.onRetryModelChange(value ?? "")}
                disablePortal
                options={props.models}
                sx={{ width: 300 }}
                renderInput={(params) => (
                    <TextField {...params} label="Retry model" />
                )}
            />

            <Typography>Languages:</Typography>
            <Autocomplete
                disablePortal
                onChange={(_, value) => props.onLanguageChange(value ?? "")}
                options={props.language}
                sx={{width: 300}}
                renderInput={(params) => <TextField {...params} label={props.selectedLangue}/>}
            />
            <OutputButton />
        </Stack>
    )
}

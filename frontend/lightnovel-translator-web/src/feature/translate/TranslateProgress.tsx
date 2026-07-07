import {Box, Button, LinearProgress, Stack, Typography} from "@mui/material";
import TranslateIcon from '@mui/icons-material/Translate';
import {useState} from "react";
import {useTranslationStore} from "../../store/tranlateStore.ts";

export default function TranslateProgress() {
    const translate = useTranslationStore(x => x.isTranslating);
    //const setTranslete = useTranslationStore(x => x.setIsTranslating);
    const [currentChunk, /*setCurrentChunk*/] = useState(50);
    const maxChunk = 100;
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
                <Button variant="outlined" startIcon={<TranslateIcon />} loading={translate}>
                    Translete...
                </Button>
                <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ mr: 1 }}
                >
                    Translate status
                </Typography>
                <Box sx={{ width: '100%', mr: 1 }}>
                <LinearProgress
                    variant="determinate"
                    aria-labelledby= "translete"
                    aria-valuetext= "Translating"
                    min= {1}
                    max={maxChunk}
                    value={currentChunk}
                />
                    <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                        chunk {currentChunk} / {maxChunk}
                    </Typography>
                </Box>
            </Stack>
        </Box>
    )
}

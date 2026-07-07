import {Box, Card, Chip} from "@mui/material";
import {useEffect, useState} from "react";
import {getModels} from "./api.ts";


export default function ModelsCard() {
    const [models, setModels] = useState(["Not Instaled"]);

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
    },[])
    return (
        <Card variant="outlined">
            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 ,        maxHeight: 350,
                overflowY: "auto"}}>
                {models.map(model => (
                    <Chip key={model} label={model} />
                ))}
            </Box>
        </Card>
    )
}

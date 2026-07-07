import StatusCard from "../feature/Ollama/StatusCard.tsx";
import {Container, Grid} from "@mui/material";
import ModelsCard from "../feature/Ollama/ModelsCard.tsx";


export default function Dashboard() {
    return (
        <Container>
            <Grid container spacing={3}>
                <Grid size={{ xs: 12, md: 6 }}>
                    <StatusCard />
                </Grid>

                <Grid size={{ xs: 12, md: 6 }}>
                    <ModelsCard />
                </Grid>
            </Grid>
        </Container>
    )
}

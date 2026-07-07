import {Container, Grid} from "@mui/material";
import ZoneFile from "../feature/file/ZoneFile.tsx";
import TranslateOutput from "../feature/translate/TranslateOutput.tsx";
import TranslateProgress from "../feature/translate/TranslateProgress.tsx";

export default function Translate() {
    return (
        <Container>
            <Grid container spacing={3}>
                <Grid size={{ xs: 12, md: 6 }}>
                    <ZoneFile />
                </Grid>

                <Grid size={{ xs: 12, md: 6 }}>
                    <TranslateOutput />
                    <TranslateProgress />
                </Grid>
            </Grid>
        </Container>
    )
}

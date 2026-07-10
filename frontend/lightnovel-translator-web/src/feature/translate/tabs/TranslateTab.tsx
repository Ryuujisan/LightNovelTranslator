import {Grid} from "@mui/material";
import ZoneFile from "../../file/ZoneFile.tsx";
import {TranslateOutput} from "../TranslateOutput.tsx";
import TranslateProgress from "../TranslateProgress.tsx";

export default function TranslateTab() {
    return (
        <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 6 }}>
                <ZoneFile />
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
                <TranslateOutput />
                <TranslateProgress />
            </Grid>
        </Grid>
    )
}

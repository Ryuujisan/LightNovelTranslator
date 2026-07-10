import {Button, Typography} from "@mui/material";
import {useTranslationStore} from "../../../store/tranlateStore.ts";
import {getOutputPath} from "../../file/api.ts";

export default function OutputButton() {

    const loading = useTranslationStore(x => x.isLoading);
    const setLoading = useTranslationStore(x => x.setIsLoading)
    const SetOutputPath = useTranslationStore(x => x.setOutputPath);
    const outputPath = useTranslationStore(x => x.outputPath);
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
        <>
            <Typography>Output Path:</Typography>
            <Button variant="outlined" loading={loading} onClick={selectPathOnClick}>{outputPath}</Button>
        </>
    )
}

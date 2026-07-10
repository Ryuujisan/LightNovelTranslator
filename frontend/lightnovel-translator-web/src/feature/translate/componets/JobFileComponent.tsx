import { Box, Checkbox, Stack, Typography, Chip } from "@mui/material";
import type {JobFile} from "../type.ts";


type Props = {
    file: JobFile;
    checked: boolean;
    onCheckedChange: (checked: boolean) => void;
    index: number;
};

function getStatusColor(status: string) {
    switch (status) {
        case "Completed":
            return "success";

        case "Failed":
            return "error";

        case "Running":
            return "warning";

        case "Pending":
            return "default";

        default:
            return "default";
    }
}
export default function JobFileComponent({
                                             file,
                                             checked,
                                             onCheckedChange,
                                             index,
                                         }: Props) {
    return (
        <Box
            sx={{
                px: 2,
                py: 1,
                borderRadius: 1,
                backgroundColor:
                    index % 2 === 0
                        ? "rgba(255,255,255,0.03)"
                        : "rgba(255,255,255,0.07)",
            }}
        >
            <Stack direction="row" spacing={2} sx={{
                alignItems: "center",
            }}>
                <Checkbox
                    checked={checked}
                    onChange={(e) => onCheckedChange(e.target.checked)}
                />

                <Typography sx={{ flex: 1 }}>{file.fileName}</Typography>

                <Chip
                    size="small"
                    label={file.status}
                    color={getStatusColor(file.status)}
                    variant="outlined"
                />
            </Stack>
        </Box>
    );
}
import {Box, LinearProgress, Typography} from "@mui/material";

type LinearChunkProgressProps = {
    current: number;
     total: number;
     title: string;
}
export default function LinearChunkProgress(props: LinearChunkProgressProps) {
    return (
        <Box sx={{ width: '100%', mr: 1 }}>
            <LinearProgress
                variant="determinate"
                aria-labelledby= "translete"
                aria-valuetext= "Translating"
                min= {0}
                max={props.total}
                value={props.current}
            />
            <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                chunk {props.current} / {props.total}
            </Typography>
        </Box>
    )
}

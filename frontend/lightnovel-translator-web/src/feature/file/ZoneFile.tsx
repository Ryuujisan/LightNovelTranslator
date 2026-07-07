import { Box, Typography, List, ListItem } from "@mui/material";
import { useDropzone } from "react-dropzone";
import { useState } from "react";
import {postResolvePath, postUploadFile} from "./api.ts";
import {http} from "../../shared/api/http.ts";

export default function ZoneFile() {
    const [files, setFiles] = useState<File[]>([]);

    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        accept: {
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document": [".docx"],
        },
        multiple: true,
        onDrop: acceptedFiles => {
            setFiles(acceptedFiles);
            resolvePath();
        },
    });

    async function resolvePath() {

        const data = await postUploadFile(files);
        console.log(data);
    }

    return (
        <Box>
            <Box
                {...getRootProps()}
                sx={{
                    border: "2px dashed",
                    borderColor: isDragActive ? "primary.main" : "divider",
                    borderRadius: 2,
                    p: 6,
                    textAlign: "center",
                    cursor: "pointer",
                    backgroundColor: "background.paper",
                }}
            >
                <input {...getInputProps()} />

                <Typography variant="h6">
                    {isDragActive ? "Drop files here" : "Drop .docx files here"}
                </Typography>

                <Typography color="text.secondary">
                    or click to select
                </Typography>
            </Box>

            <List>
                {files.map(file => (
                    <ListItem key={file.name}>
                        {file.name}
                    </ListItem>
                ))}
            </List>
        </Box>
    );
}
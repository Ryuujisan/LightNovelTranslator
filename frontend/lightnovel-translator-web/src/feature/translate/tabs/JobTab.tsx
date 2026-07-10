import {Alert, Autocomplete, Box, Button, Grid, Stack, TextField, Typography} from "@mui/material";
import DeleteIcon from '@mui/icons-material/Delete';
import {useEffect, useState} from "react";
import JobFileComponent from "../componets/JobFileComponent.tsx";
import ZoneFile from "../../file/ZoneFile.tsx";
import JobSettings from "../JobSettings.tsx";
import type {Job, JobDto, JobFile} from "../type.ts";
import {fetchJob, fetchJobs} from "../api.ts";
import {useTranslationStore} from "../../../store/tranlateStore.ts";
import JobProgress from "../JobProgress.tsx";

export default function JobTab() {

    const [jobs, setJobs] = useState<JobDto[]>([]);
    const selectedJob = useTranslationStore(x => x.selectedJob);
    const setSelectedJob = useTranslationStore(x => x.setSelectedJob);
    const selectableFiles = useTranslationStore(x => x.selectableJobFiles)
    const setSelectableFiles = useTranslationStore(x => x.setSelectableJobFiles);
    async function selectedJobsOnClick(value: string) {
        try {
            console.log("id: " + value);
            const job = await fetchJob(value) as Job;
            console.log(job);
            setSelectedJob(job);

        } catch (e) {
            console.log(e);
        }
    }



    function toggleFile(fileName: JobFile, checked: boolean) {
        setSelectableFiles(prev =>
            checked
                ? [...prev, fileName]
                : prev.filter(x => x !== fileName)
        );
    }

    useEffect(() => {
        async function fetch() {
            try {
                const jobs = await fetchJobs() as JobDto[];
                console.log(jobs);
                if(jobs.length === 0) {
                    setJobs([])
                } else {
                    setJobs(jobs)
                }

            } catch (e) {
                console.log(e);
            }
        }
        fetch();
    },[])

    return (
        <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 6 }}>
                <Stack spacing={1}>
                    <Stack direction="row" spacing={1}>
                        <Autocomplete
                            onChange={(_, value ) => selectedJobsOnClick(value?.id ?? "")}
                            disablePortal
                            options={jobs}
                            getOptionLabel={(option) => option.name}
                            sx={{width: 300}}
                            renderInput={(params) => <TextField {...params} label={"Jobs id"}
                            />}
                        />
                        <Button variant="outlined" color="error"><DeleteIcon/></Button>
                    </Stack>

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
                        <Typography component="legend">File</Typography>
                        <Stack spacing={0.5}
                               sx={{ display: "flex",
                                   gap: 1 ,
                                   maxHeight: 350,
                                   overflowY: "auto"}}>
                            {selectedJob?.inputPath.map((file, index) => (
                                <JobFileComponent
                                    key={file.fileName + index}
                                    file={file}
                                    index={index}
                                    checked={selectableFiles.some(x => x.fileName === file.fileName)}
                                    onCheckedChange={(checked) => toggleFile(file, checked)}
                                />
                            ))}
                        </Stack>
                    </Box>
                    {(selectedJob === null) ?
                    <Alert severity="warning">Select job to see files</Alert> :
                    <ZoneFile jobId={selectedJob.id} onUploaded={async () => await selectedJobsOnClick(selectedJob.id)}/>}
                    <Button
                        startIcon={<DeleteIcon />}
                        color="error"
                    >
                        Delete selected
                    </Button>
                </Stack>
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
                <JobSettings />
                <JobProgress />
            </Grid>
        </Grid>
    )
}

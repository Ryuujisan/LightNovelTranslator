import {Navbar} from "./Navbar.tsx";
import { Outlet } from "react-router-dom";
import {Box, Stack} from "@mui/material";
import {useEffect} from "react";
import {useSocketStore} from "../store/socketStore.ts";


export default function App() {
    const initSignalR = useSocketStore(state => state.initSignalR);

    useEffect(() => {
        initSignalR();
    }, [initSignalR]);

    return (
        <Box
            sx={{
              maxWidth: 1400,
              mx: "auto",
              mt: 4,
              px: 3,
              py: 4,
            }}
        >
            <Stack spacing={2}>
                <Navbar />
                <Outlet />
            </Stack>
        </Box>
    )
}

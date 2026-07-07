import {Navbar} from "./Navbar.tsx";
import { Outlet } from "react-router-dom";
import {Box, Stack} from "@mui/material";


export default function App() {
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

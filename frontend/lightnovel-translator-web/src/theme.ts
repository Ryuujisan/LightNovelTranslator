import { createTheme } from "@mui/material";

export const theme = createTheme({
    palette: {
        mode: "dark",
        primary: {
            main: "#9c27b0",
            light: "#ce93d8",
            dark: "#6a1b9a",
            contrastText: "#ffffff",
        },
        secondary: {
            main: "#7e57c2",
            light: "#b39ddb",
            dark: "#512da8",
            contrastText: "#ffffff",
        },
        background: {
            default: "#12091f",
            paper: "#1d102e",
        },
    },
});
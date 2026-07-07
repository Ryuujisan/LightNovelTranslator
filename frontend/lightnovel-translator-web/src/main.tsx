
import { createRoot } from 'react-dom/client'
import {RouterProvider} from "react-router/dom";
import './index.css'
import {CssBaseline, ThemeProvider} from "@mui/material";
import {theme} from "./theme.ts";
import {ToastContainer} from "react-toastify";
import {StrictMode} from "react";
import {routes} from "./app/Routes.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>

        <ThemeProvider theme={theme}>
            <CssBaseline />
            <RouterProvider router={routes} />
            <ToastContainer position="bottom-right" theme='colored'/>
        </ThemeProvider>
    </StrictMode>
)

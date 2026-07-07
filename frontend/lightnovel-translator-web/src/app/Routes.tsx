import {createBrowserRouter} from "react-router-dom";
import App from "../layaout/App.tsx";
import Dashboard from "../pages/Dashboard.tsx";
import Translate from "../pages/Translate.tsx";
import Setting from "../pages/Setting.tsx";
export const routes = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            {index: true, element: <Dashboard/>},
            {path:'translate', element: <Translate/>},
            {path:'settings', element: <Setting/>}
        ]
    }
])
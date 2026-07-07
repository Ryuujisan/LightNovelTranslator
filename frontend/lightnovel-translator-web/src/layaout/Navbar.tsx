import {AppBar, BottomNavigation, BottomNavigationAction} from "@mui/material";
import {useState} from "react";
import DashboardIcon from '@mui/icons-material/Dashboard';
import TranslateIcon from '@mui/icons-material/Translate';
import SettingsIcon from '@mui/icons-material/Settings';
import { useNavigate } from "react-router-dom";


export const Navbar = () => {

    const [value, setValue] = useState(0);
    const navigationItems = [
        {
            label: "Dashboard",
            path: "/",
            icon: <DashboardIcon />
        },
        {
            label: "Translate",
            path: "/translate",
            icon: <TranslateIcon />
        },
        {
            label: "Settings",
            path: "/settings",
            icon: <SettingsIcon />
        }
    ];
    const navigate = useNavigate();
    const handleNavigationChange = (_event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
        navigate(navigationItems[newValue].path);
    };
    return (
        <AppBar position="static">
            <BottomNavigation
                showLabels
                value={value}
                onChange={handleNavigationChange}
            >
                {navigationItems.map(item => (
                    <BottomNavigationAction
                        key={item.path}
                        label={item.label}
                        icon={item.icon}
                    />
                ))}
            </BottomNavigation>
        </AppBar>
    )
};


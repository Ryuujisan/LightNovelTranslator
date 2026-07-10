import {Container, Tab, Tabs} from "@mui/material";
import {useState} from "react";
import JobTab from "../feature/translate/tabs/JobTab.tsx";
import TranslateTab from "../feature/translate/tabs/TranslateTab.tsx";


export default function Translate() {
    type Tab ={
        label: string;
        tabView: React.ReactNode
    }
    const tabs : Tab[] = [
        {
            label: "Translate",
            tabView : <TranslateTab/>
        },
        {   label: "Jobs",
            tabView: <JobTab />
        }
    ]

    const [value, setValue] = useState(tabs[0].label);

    const handleChange = (_: React.SyntheticEvent, newValue: string) => {
        setValue(newValue);
    };

    const currentTab = tabs.find(x => x.label === value);
    return (
        <Container>
            <Tabs
                value={value}
                onChange={handleChange}
            >
                {tabs.map(tab => (
                    <Tab
                        key={tab.label}
                        value={tab.label}
                        label={tab.label}
                    />
                ))}
            </Tabs>

            {currentTab?.tabView}

        </Container>
    )
}

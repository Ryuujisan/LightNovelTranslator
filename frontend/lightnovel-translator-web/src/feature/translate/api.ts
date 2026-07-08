
import type {TranslationJobRequest} from "./type.ts";
import {http} from "../../shared/api/http.ts";

export async function postTranslate(job : TranslationJobRequest){
    console.log(job);
    const response = await http.post("/translate/job", job);
    return response.data;
}
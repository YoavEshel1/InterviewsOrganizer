import { Position } from "./position";

export interface Company {
    id : string; 
    name: string;
    companyInfo?: string;    
    interviews: Position[];    
}
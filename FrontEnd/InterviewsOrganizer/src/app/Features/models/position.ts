import { Interview } from "./interview";

export interface Position {
     id :string;
     title: string; 
     applyDate: string;
     status: string;
     notes?: string;
     companyId: string;      
     interviews?: Interview[];
}
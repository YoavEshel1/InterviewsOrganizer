export interface Interview {
    id : string; 
    date: Date;  
    feeling: number; // 1-5  
    positionId: string;
    interviewer: string;
    notes?: string;  
}
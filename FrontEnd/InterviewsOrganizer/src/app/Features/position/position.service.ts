import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Position, PositionStatus } from '../models/position';

export interface CreatePositionRequest {
  title: string;
  applyDate: string | null;
  status: PositionStatus;
  notes: string;
}

@Injectable({ providedIn: 'root' })
export class PositionService {
  private readonly http = inject(HttpClient);

  create(companyId: string, request: CreatePositionRequest): Observable<Position> {
    return this.http.post<Position>(`/api/companies/${companyId}/positions`, request);
  }
}

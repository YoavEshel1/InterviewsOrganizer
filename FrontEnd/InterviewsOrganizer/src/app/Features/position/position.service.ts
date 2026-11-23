import { HttpClient, httpResource } from '@angular/common/http';
import { inject, Injectable, Signal } from '@angular/core';
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

  private readonly positionsUrl = '/api/companies';

//changing the token will trigger refresh of positions list
  getPositions(companyId: Signal<string>, refreshToken: Signal<number>) {
    return httpResource<Position[]>(() => {
      const id = companyId();
      refreshToken();
      return id
        ? { url: `${this.positionsUrl}/${encodeURIComponent(id)}/positions` }
        : undefined;
    }, { defaultValue: [] });
  }

  create(companyId: string, request: CreatePositionRequest): Observable<Position> {
    return this.http.post<Position>(
      `${this.positionsUrl}/${encodeURIComponent(companyId)}/positions`,
      request,
    );
  }
}

import { Component, inject, input, signal } from '@angular/core';
import { PositionService } from '../position.service';
import { positionStatusLabel } from '../../models/position';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-position-list',
  imports: [DatePipe],
  templateUrl: './position-list.html',
  styleUrl: './position-list.scss',
})
export class PositionList {
  private readonly positionService = inject(PositionService);
  protected readonly statusLabel = positionStatusLabel;

  readonly companyId = input.required<string>();
  // changing the token will trigger refresh of positions list
  //token number is not sent to the server, it is only used to trigger refresh of the positions list
  private readonly refreshToken = signal(0);
  readonly positions = this.positionService.getPositions(this.companyId, this.refreshToken);

  reload(): void {
    this.refreshToken.update((version) => version + 1);
  }
}

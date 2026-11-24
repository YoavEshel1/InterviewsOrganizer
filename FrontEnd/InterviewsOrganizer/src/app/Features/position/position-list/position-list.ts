import { Component, inject, input, linkedSignal, signal } from '@angular/core';
import { PositionService } from '../position.service';
import { PositionItem } from '../position-item/position-item';

@Component({
  selector: 'app-position-list',
  imports: [PositionItem],
  templateUrl: './position-list.html',
  styleUrl: './position-list.scss',
})
export class PositionList {
  private readonly positionService = inject(PositionService);

  readonly companyId = input.required<string>();
  protected readonly selectedPositionId = linkedSignal<string | null>(() => {
    this.companyId();
    return null;
  });
  // changing the token will trigger refresh of positions list
  //token number is not sent to the server, it is only used to trigger refresh of the positions list
  private readonly refreshToken = signal(0);
  readonly positions = this.positionService.getPositions(this.companyId, this.refreshToken);

  protected selectPosition(positionId: string): void {
    this.selectedPositionId.set(positionId);
  }

  reload(): void {
    this.refreshToken.update((version) => version + 1);
  }
}

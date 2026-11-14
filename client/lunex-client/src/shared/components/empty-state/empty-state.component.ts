import { Component, input } from '@angular/core';
import { CardComponent } from '../card/card.component';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CardComponent],
  template: `
    <app-card variant="bordered" padding="lg">
      <div class="empty-state">
        <div class="empty-state__icon" aria-hidden="true">
          <ng-content select="[empty-icon]"></ng-content>
        </div>
        <h2>{{ title() }}</h2>
        <p>{{ description() }}</p>
        <div class="empty-state__actions">
          <ng-content select="[empty-actions]"></ng-content>
        </div>
      </div>
    </app-card>
  `,
})
export class EmptyStateComponent {
  title = input.required<string>();
  description = input.required<string>();
}

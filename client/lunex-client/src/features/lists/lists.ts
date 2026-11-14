import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../core/services/account-service';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { CardComponent } from '../../shared/components/card/card.component';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [CommonModule, RouterLink, ButtonComponent, CardComponent, PageHeaderComponent, EmptyStateComponent],
  templateUrl: './lists.html',
  styleUrl: './lists.css',
})
export class ListsComponent {
  accountService = inject(AccountService);
}
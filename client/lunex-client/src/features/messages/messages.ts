import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../core/services/account-service';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { CardComponent } from '../../shared/components/card/card.component';
import { AvatarComponent } from '../../shared/components/avatar/avatar.component';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';

interface Conversation {
  id: string;
  name: string;
  lastMessage: string;
  time: string;
  unread: number;
  online: boolean;
  avatar?: string;
}

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [CommonModule, RouterLink, ButtonComponent, CardComponent, AvatarComponent, PageHeaderComponent, EmptyStateComponent],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class MessagesComponent {
  accountService = inject(AccountService);

  // Mock conversations for demo
  conversations: Conversation[] = [
    {
      id: '1',
      name: 'Sarah M.',
      lastMessage: 'Hey! I saw your profile and...',
      time: '2 min ago',
      unread: 3,
      online: true,
    },
    {
      id: '2',
      name: 'Alex K.',
      lastMessage: 'Thanks for the match! 😊',
      time: '1 hour ago',
      unread: 0,
      online: true,
    },
    {
      id: '3',
      name: 'Jordan P.',
      lastMessage: 'What are you up to this weekend?',
      time: '3 hours ago',
      unread: 1,
      online: false,
    },
    {
      id: '4',
      name: 'Taylor R.',
      lastMessage: 'Your photos are amazing!',
      time: 'Yesterday',
      unread: 0,
      online: false,
    },
  ];
}
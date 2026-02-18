import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { Nav } from '../layout/nav/nav';
import { AccountService } from '../core/services/account-service';
import { Home } from '../features/home/home';
import { User } from '../types/User';

@Component({
  selector: 'app-root',
  imports: [Nav, Home],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);

  private readonly baseUrl = 'https://localhost:5001';

  protected readonly title = signal('Lunex');
  protected members = signal<User[]>([]);

  async ngOnInit() {
    this.members.set((await this.getMembers()) ?? []);
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user = localStorage.getItem('user');
    if (!user) return;
    this.accountService.currentUser.set(JSON.parse(user));
  }

  async getMembers() {
    try {
      return await lastValueFrom(this.http.get<User[]>(`${this.baseUrl}/api/members`));
    } catch (error) {
      console.error('Error fetching members:', error);
      return null;
    }
  }
}

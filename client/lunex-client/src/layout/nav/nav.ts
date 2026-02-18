import { Component, inject, signal } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-nav',
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  public accountService = inject(AccountService);
  protected cred: any = {};
  login() {
    this.accountService.login(this.cred).subscribe({
      next: (result) => {
        console.log('Login successful:', result);
        this.cred = {};
      },
      error: (error) => console.log('Login failed: ' + error.message),
    });
  }
  logout() {
    this.accountService.logout();
  }
}

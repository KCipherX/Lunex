import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MemberService } from '../../../core/services/member-service';
import { Member, PaginatedResult } from '../../../types/User';
import { AccountService } from '../../../core/services/account-service';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [CommonModule, RouterLink, ButtonComponent, CardComponent, PageHeaderComponent, EmptyStateComponent],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberListComponent implements OnInit {
  private memberService = inject(MemberService);
  private accountService = inject(AccountService);

  members = signal<Member[]>([]);
  loading = signal(false);
  pagination = signal<PaginatedResult<Member> | null>(null);
  currentPage = signal(1);
  pageSize = 12;
  error = signal<string | null>(null);

  currentUser = this.accountService.currentUser;

  isCurrentUser = computed(() => (member: Member) => 
    this.currentUser()?.id === member.id
  );

  async ngOnInit() {
    await this.loadMembers();
  }

  async loadMembers() {
    this.loading.set(true);
    this.error.set(null);
    try {
      const result = await this.memberService.getMembers(this.currentPage(), this.pageSize).toPromise();
      if (result) {
        this.members.set(result.items);
        this.pagination.set(result);
      }
    } catch (error) {
      console.error('Error loading members:', error);
      this.error.set('Failed to load members. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= (this.pagination()?.totalPages ?? 1)) {
      this.currentPage.set(page);
      this.loadMembers();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  onPageSizeChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.pageSize = parseInt(target.value, 10);
    this.currentPage.set(1);
    this.loadMembers();
  }

  calculateAge(dateOfBirth: string): number {
    const birthDate = new Date(dateOfBirth);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }

  getPageNumbers(): number[] {
    const total = this.pagination()?.totalPages ?? 1;
    const current = this.currentPage();
    const pages: number[] = [];
    
    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
    } else {
      pages.push(1);
      if (current > 3) pages.push(-1); // ellipsis
      for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
        pages.push(i);
      }
      if (current < total - 2) pages.push(-1); // ellipsis
      pages.push(total);
    }
    return pages;
  }

  protected Math = Math;
}
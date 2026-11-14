import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MemberService } from '../../../core/services/member-service';
import { Member, Photo, MemberUpdateRequest } from '../../../types/User';
import { AccountService } from '../../../core/services/account-service';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { InputComponent } from '../../../shared/components/input/input.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { AvatarComponent } from '../../../shared/components/avatar/avatar.component';
import { LoadingComponent } from '../../../shared/components/loading/loading.component';
import { ModalComponent } from '../../../shared/components/modal/modal.component';

@Component({
  selector: 'app-member-detailed',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent, InputComponent, CardComponent, AvatarComponent, LoadingComponent, ModalComponent],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailedComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private memberService = inject(MemberService);
  private accountService = inject(AccountService);

  member = signal<Member | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  editing = signal(false);
  uploading = signal(false);
  deletePhotoModalOpen = signal(false);
  photoToDelete = signal<string | null>(null);

  // Edit form
  editForm: MemberUpdateRequest = {
    displayName: '',
    description: '',
    city: '',
    country: '',
  };

  currentUser = this.accountService.currentUser;

  isCurrentUser = computed(() => {
    const user = this.currentUser();
    const mem = this.member();
    return user?.id === mem?.id;
  });

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      await this.loadMember(id);
    }
  }

  async loadMember(id: string) {
    this.loading.set(true);
    this.error.set(null);
    try {
      const member = await this.memberService.getMemberById(id).toPromise();
      if (member) {
        this.member.set(member);
        this.initializeEditForm(member);
      } else {
        this.error.set('Member not found');
      }
    } catch (error) {
      console.error('Error loading member:', error);
      this.error.set('Failed to load member');
    } finally {
      this.loading.set(false);
    }
  }

  initializeEditForm(member: Member) {
    this.editForm = {
      displayName: member.name,
      description: member.description,
      city: member.city,
      country: member.country,
    };
  }

  toggleEdit() {
    if (this.editing()) {
      this.initializeEditForm(this.member()!);
    }
    this.editing.set(!this.editing());
  }

  async saveProfile() {
    if (!this.member()) return;
    this.loading.set(true);
    try {
      const updatedMember = await this.memberService.updateMemberProfile(this.editForm).toPromise();
      if (updatedMember) {
        this.member.set(updatedMember);
        this.editing.set(false);
      }
    } catch (error) {
      console.error('Error updating profile:', error);
      this.error.set('Failed to update profile');
    } finally {
      this.loading.set(false);
    }
  }

  async onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file
      if (!this.validateFile(file)) {
        input.value = '';
        return;
      }

      this.uploading.set(true);
      try {
        const photo = await this.memberService.uploadPhoto(file).toPromise();
        if (photo && this.member()) {
          const updatedMember = { 
            ...this.member()!, 
            photos: [...this.member()!.photos, photo],
            imageUrl: this.member()!.imageUrl || photo.url
          };
          this.member.set(updatedMember);
        }
      } catch (error) {
        console.error('Error uploading photo:', error);
        this.error.set('Failed to upload photo');
      } finally {
        this.uploading.set(false);
        input.value = '';
      }
    }
  }

  validateFile(file: File): boolean {
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    const maxSize = 10 * 1024 * 1024; // 10MB

    if (!allowedTypes.includes(file.type)) {
      this.error.set('Invalid file type. Allowed: JPG, PNG, GIF, WebP');
      return false;
    }

    if (file.size > maxSize) {
      this.error.set('File size must be less than 10MB');
      return false;
    }

    return true;
  }

  confirmDeletePhoto(photoId: string) {
    this.photoToDelete.set(photoId);
    this.deletePhotoModalOpen.set(true);
  }

  async deletePhoto() {
    const photoId = this.photoToDelete();
    if (!photoId || !this.member()) return;

    try {
      await this.memberService.deletePhoto(photoId).toPromise();
      const updatedMember = {
        ...this.member()!,
        photos: this.member()!.photos.filter((p) => p.id !== photoId),
      };
      this.member.set(updatedMember);
      this.deletePhotoModalOpen.set(false);
      this.photoToDelete.set(null);
    } catch (error) {
      console.error('Error deleting photo:', error);
      this.error.set('Failed to delete photo');
    }
  }

  async setMainPhoto(photoId: string) {
    try {
      await this.memberService.setMainPhoto(photoId).toPromise();
      await this.loadMember(this.member()!.id);
    } catch (error) {
      console.error('Error setting main photo:', error);
      this.error.set('Failed to set main photo');
    }
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

  getMainPhoto(): Photo | undefined {
    return this.member()?.photos?.[0];
  }

  getOtherPhotos(): Photo[] {
    const photos = this.member()?.photos || [];
    return photos.slice(1);
  }

  trackByPhotoId(index: number, photo: Photo): string {
    return photo.id;
  }
}
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService } from '../../services/profile.service';
import { AlertsService } from '../../services/alerts.service';
import { AuthService } from '../../services/auth';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './profile.html',
    styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
    private fb = inject(FormBuilder);
    private profileService = inject(ProfileService);
    private alertService = inject(AlertsService);
    private authService = inject(AuthService);


    isLoading = true;

    profileForm: FormGroup = this.fb.group({
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        homeAddress: ['', Validators.required],
        preferAccessiblePaths: [false],
        childrenAges: this.fb.array([])
    });

    get childrenAgesArray(): FormArray {
        return this.profileForm.get('childrenAges') as FormArray;
    }

    ngOnInit() {
        this.loadProfile();
    }

    loadProfile() {
        this.profileService.getProfile().subscribe({
            next: (user) => {
                // מילוי השדות הפשוטים
                this.profileForm.patchValue({
                    firstName: user.firstName,
                    lastName: user.lastName,
                    email: user.email,
                    homeAddress: user.homeAddress,
                    preferAccessiblePaths: user.preferAccessiblePaths
                });

                // מילוי ה-FormArray של הגילאים - חייבים להוסיף control לכל גיל קיים
                this.childrenAgesArray.clear();
                (user.childrenAges || []).forEach((age: number) => {
                    this.childrenAgesArray.push(
                        this.fb.control(age, [Validators.required, Validators.min(0), Validators.max(18)])
                    );
                });

                this.isLoading = false;
            },
            error: (err) => {
                console.error('Error loading profile:', err);
                this.alertService.showError('לא הצלחנו לטעון את הפרופיל שלך');
                this.isLoading = false;
            }
        });
    }

    addChild() {
        this.childrenAgesArray.push(
            this.fb.control(null, [Validators.required, Validators.min(0), Validators.max(18)])
        );
    }

    removeChild(index: number) {
        this.childrenAgesArray.removeAt(index);
    }

    onSubmit() {
        if (this.profileForm.invalid) {
            this.alertService.showError('נא למלא את כל השדות בצורה תקינה');
            return;
        }

        const payload = {
            ...this.profileForm.value,
            childrenAges: this.profileForm.value.childrenAges.map((age: any) => Number(age))
        };

        this.profileService.updateProfile(payload).subscribe({
            next: () => {
                this.alertService.showSuccess('הפרופיל עודכן בהצלחה!');
                this.authService.updateCurrentUserData({        // : עדכון מיידי של השם בנאבבר
                    firstName: payload.firstName,
                    lastName: payload.lastName
                });
            },
            error: (err) => {
                console.error('Update Error:', err);
                this.alertService.showError('שגיאה בעדכון הפרופיל');
            }
        });
    }
}
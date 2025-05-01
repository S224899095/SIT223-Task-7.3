import { Component, Input, OnInit } from '@angular/core';
import { MemberModel } from '../../../Models/member';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { environment } from '../../../../environments/environment.development';
import { AccountService } from '../../../services/account.service';
import { take } from 'rxjs';
import { UserModel } from '../../../Models/userModel';
import { CommonModule, NgClass } from '@angular/common';
import { MembersService } from '../../../services/members.service';
import { PhotoModel } from '../../../Models/photo';

@Component({
    selector: 'app-photo-editor',
    standalone: true,
    imports: [FileUploadModule, NgClass, CommonModule],
    templateUrl: './photo-editor.component.html',
    styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent implements OnInit {

    @Input() public member?: MemberModel;
    public uploader?: FileUploader;
    public hasBaseDropzoneOver = false;
    private baseURL = environment.apiUrl;
    private user?: UserModel | null;

    constructor(private accountService: AccountService, private memberService: MembersService) {
        this.accountService.getCurrentUser().pipe(take(1)).subscribe(user => this.user = user);
    }
    public ngOnInit(): void {
        this.initialiseUploader();
    }

    public fileOverBase(e: any) {
        this.hasBaseDropzoneOver = e;
    }

    public initialiseUploader() {
        this.uploader = new FileUploader({
            url: this.baseURL + 'user/add-photo',
            authToken: 'Bearer ' + this.user?.token,
            isHTML5: true,
            allowedFileType: ['image'],
            removeAfterUpload: true,
            autoUpload: false,
            maxFileSize: 10 * 1024 * 1024
        });

        this.uploader.onAfterAddingFile = (File) => {
            File.withCredentials = false;
        }

        this.uploader.onSuccessItem = (item, response, status, headerser) => {
            if (response) {
                const photo = JSON.parse(response);
                this.member?.photos.push(photo);
                if(photo.isMain){
                    this.user!.photoUrl = photo.url;
                    this.member!.photoUrl = photo.url;
                    this.accountService.setLocalStorageUser(this.user!);
                }
            }
        }
    }

    public setMainPhoto(photo: PhotoModel) {
        this.memberService.setMainPhoto(photo.id).subscribe(() => {
            this.user!.photoUrl = photo.url;
            this.accountService.setLocalStorageUser(this.user!);
            this.member!.photoUrl = photo.url;
            this.member?.photos.forEach(p => {
                if (p.isMain) p.isMain = false;
                if (p.id === photo.id) p.isMain = true;
            })
        });
    }

    public deletePhoto(photo: PhotoModel) {
        this.memberService.deletePhoto(photo.id).subscribe(() => {
            this.member!.photos = this.member!.photos.filter(x => x.id !== photo.id);
        });
    }
}

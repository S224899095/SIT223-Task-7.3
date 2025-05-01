import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../Components/modals/confirm-dialog/confirm-dialog.component';
import { Observable, Observer, Subscriber } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ConfirmService {

    bsModelRef?: BsModalRef

    constructor(private modalService: BsModalService) { }

    confirm(title = 'confirmation', message = 'Are you sure you want to do this?', btnOkText = 'Ok', btnCancelText = 'Cancel') : Observable<boolean> {
        const config = {
            initialState : {
                title,
                message,
                btnOkText,
                btnCancelText
            }
        }

        this.bsModelRef = this.modalService.show(ConfirmDialogComponent, config);
        return new Observable<boolean>(this.getResult());
    }

    private getResult(){
        return (observer: Subscriber<boolean>) => {
            const subscription = this.bsModelRef?.onHidden?.subscribe(() => {
                observer.next(this.bsModelRef?.content.result);
                observer.complete();
            });

            return {
                unsubscribe(){
                    subscription?.unsubscribe();
                }
            }
        }
    }
}

